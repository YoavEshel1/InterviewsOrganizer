import { TestBed } from '@angular/core/testing';
import { vi } from 'vitest';
import { CompanyService } from '../company/companyService';
import { StartPage } from './start-page';

describe('StartPage', () => {
  const companyService = { startCreatingCompany: vi.fn() };

  function createComponent() {
    TestBed.configureTestingModule({
      imports: [StartPage],
      providers: [{ provide: CompanyService, useValue: companyService }],
    });
    const fixture = TestBed.createComponent(StartPage);
    fixture.detectChanges();
    return fixture;
  }

  beforeEach(() => companyService.startCreatingCompany.mockClear());

  it('explains the Company > Positions > Interviews hierarchy', () => {
    const text: string = createComponent().nativeElement.textContent;

    expect(text).toContain('Company');
    expect(text).toContain('Positions');
    expect(text).toContain('Interviews');
  });

  it('starts company creation from the call-to-action', () => {
    const fixture = createComponent();
    const cta: HTMLButtonElement = fixture.nativeElement.querySelector('button.cta');

    expect(cta.textContent).toContain('Add Your First Company');
    cta.click();

    expect(companyService.startCreatingCompany).toHaveBeenCalledOnce();
  });
});
