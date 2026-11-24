import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { PositionList } from './position-list';

describe('PositionList', () => {
  let component: PositionList;
  let fixture: ComponentFixture<PositionList>;
  let http: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PositionList],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    })
    .compileComponents();

    fixture = TestBed.createComponent(PositionList);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('companyId', 'c1');
    http = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
    http.expectOne('/api/companies/c1/positions').flush([]);
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('reloads positions for its company', () => {
    component.reload();
    fixture.detectChanges();

    http.expectOne('/api/companies/c1/positions').flush([]);
  });

  it('renders positions and selects only the clicked item', () => {
    component.positions.value.set([
      {
        id: 'p1',
        title: 'Engineer',
        companyId: 'c1',
        applyDate: '2026-11-11',
        status: 0,
      },
      {
        id: 'p2',
        title: 'Designer',
        companyId: 'c1',
        applyDate: '',
        status: 1,
      },
    ]);
    fixture.detectChanges();

    const items: HTMLElement[] = Array.from(fixture.nativeElement.querySelectorAll('app-position-item'));
    expect(items).toHaveLength(2);
    expect(items[0].textContent).toContain('Engineer');
    expect(items[0].textContent).toContain('Applied');
    expect(items[1].textContent).toContain('Designer');

    items[0].click();
    fixture.detectChanges();
    expect(items[0].classList.contains('selected')).toBe(true);
    expect(items[1].classList.contains('selected')).toBe(false);

    items[1].click();
    fixture.detectChanges();
    expect(items[0].classList.contains('selected')).toBe(false);
    expect(items[1].classList.contains('selected')).toBe(true);
  });

  it('clears the selection when the company changes', () => {
    component.positions.value.set([
      { id: 'p1', title: 'Engineer', companyId: 'c1', applyDate: '', status: 0 },
    ]);
    fixture.detectChanges();
    const item: HTMLElement = fixture.nativeElement.querySelector('app-position-item');
    item.click();
    fixture.detectChanges();
    expect(item.classList.contains('selected')).toBe(true);

    fixture.componentRef.setInput('companyId', 'c2');
    fixture.detectChanges();
    TestBed.tick();
    fixture.detectChanges();

    expect(component['selectedPositionId']()).toBeNull();
    http.expectOne('/api/companies/c2/positions').flush([]);
  });
});
