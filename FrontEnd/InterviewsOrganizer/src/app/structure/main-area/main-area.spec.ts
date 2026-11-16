import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { CompanyService } from '../../Features/company/companyService';
import { MainArea } from './main-area';

describe('MainArea', () => {
  const showStartPage = signal(false);
  const isCreatingCompany = signal(false);
  const companyService = {
    showStartPage,
    isCreatingCompany,
    companies: signal([]),
    loadingCompanies: signal(false),
    selectedCompany: signal(null),
    startCreatingCompany: () => undefined,
    stopCreatingCompany: () => undefined,
    clearSelection: () => undefined,
  };

  function createFixture() {
    TestBed.configureTestingModule({
      imports: [MainArea],
      providers: [
        provideRouter([{ path: 'companies', children: [] }]),
        { provide: CompanyService, useValue: companyService },
      ],
    });
    const fixture = TestBed.createComponent(MainArea);
    fixture.detectChanges();
    return fixture;
  }

  beforeEach(() => {
    showStartPage.set(false);
    isCreatingCompany.set(false);
  });

  it('should create', () => {
    expect(createFixture().componentInstance).toBeTruthy();
  });

  it('shows the start page instead of the list when there are no companies', () => {
    showStartPage.set(true);
    const el: HTMLElement = createFixture().nativeElement;

    expect(el.querySelector('app-start-page')).not.toBeNull();
    expect(el.querySelector('app-company-list')).toBeNull();
  });

  it('shows the list and router outlet otherwise', () => {
    const el: HTMLElement = createFixture().nativeElement;

    expect(el.querySelector('app-start-page')).toBeNull();
    expect(el.querySelector('app-company-list')).not.toBeNull();
    expect(el.querySelector('app-company-form')).toBeNull();
  });

  it('shows the blank company form inline while creating', () => {
    isCreatingCompany.set(true);
    const el: HTMLElement = createFixture().nativeElement;

    expect(el.querySelector('app-company-form')).not.toBeNull();
    expect(el.querySelector('app-start-page')).toBeNull();
  });

  it('switches back to the start page when the last company is removed', () => {
    const fixture = createFixture();
    expect(fixture.nativeElement.querySelector('app-start-page')).toBeNull();

    showStartPage.set(true);
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('app-start-page')).not.toBeNull();
  });
});
