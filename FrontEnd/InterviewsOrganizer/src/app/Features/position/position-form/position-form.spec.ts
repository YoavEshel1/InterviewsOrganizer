import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { vi } from 'vitest';
import { POSITION_STATUS } from '../../models/position';
import { PositionService } from '../position.service';
import { PositionForm } from './position-form';

describe('PositionForm', () => {
  const positionService = { create: vi.fn() };
  const saved = { id: 'p1', title: 'Engineer', companyId: 'c1', applyDate: '2026-11-11', status: 0 };

  function createComponent() {
    TestBed.configureTestingModule({
      imports: [PositionForm],
      providers: [{ provide: PositionService, useValue: positionService }],
    });
    const fixture = TestBed.createComponent(PositionForm);
    fixture.componentRef.setInput('companyId', 'c1');
    fixture.detectChanges();
    return fixture;
  }

  beforeEach(() => positionService.create.mockReset());

  it('requires a job title and does not call the API when invalid', () => {
    const component = createComponent().componentInstance;

    component['onSubmit']();

    expect(component['form'].controls.title.hasError('required')).toBe(true);
    expect(positionService.create).not.toHaveBeenCalled();
  });

  it('creates the position for the company and emits it', () => {
    positionService.create.mockReturnValue(of(saved));
    const fixture = createComponent();
    const component = fixture.componentInstance;
    const emitted: unknown[] = [];
    fixture.componentInstance.saved.subscribe((p) => emitted.push(p));

    component['form'].patchValue({
      title: '  Engineer ',
      applyDate: new Date(2026, 10, 11),
      status: POSITION_STATUS.InProgress,
      notes: 'Referral',
    });
    component['onSubmit']();

    expect(positionService.create).toHaveBeenCalledWith('c1', {
      title: 'Engineer',
      applyDate: '2026-11-11',
      status: POSITION_STATUS.InProgress,
      notes: 'Referral',
    });
    expect(emitted).toEqual([saved]);
    expect(component['form'].controls.title.value).toBe('');
  });

  it('shows the server error and keeps the form values', () => {
    positionService.create.mockReturnValue(
      throwError(() => new HttpErrorResponse({ status: 400, error: 'Company not found.' })),
    );
    const component = createComponent().componentInstance;
    component['form'].patchValue({ title: 'Engineer' });

    component['onSubmit']();

    expect(component['errorMessage']()).toBe('Company not found.');
    expect(component['form'].controls.title.value).toBe('Engineer');
    expect(component['saving']()).toBe(false);
  });

  it('emits cancelled from the cancel button', () => {
    const fixture = createComponent();
    let cancelled = false;
    fixture.componentInstance.cancelled.subscribe(() => (cancelled = true));

    const buttons: HTMLButtonElement[] = Array.from(fixture.nativeElement.querySelectorAll('button'));
    buttons.find((b) => b.textContent?.includes('Cancel'))!.click();

    expect(cancelled).toBe(true);
  });
});
