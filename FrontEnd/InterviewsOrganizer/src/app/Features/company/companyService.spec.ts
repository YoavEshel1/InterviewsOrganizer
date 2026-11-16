import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { CompanyService } from './companyService';

describe('CompanyService start page state', () => {
  let service: CompanyService;
  let http: HttpTestingController;

  const company = { id: '1', name: 'Acme', positions: [] };

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    service = TestBed.inject(CompanyService);
    http = TestBed.inject(HttpTestingController);
  });

  async function flushCompanies(body: unknown[]): Promise<void> {
    TestBed.tick();
    http.expectOne('/api/companies').flush(body);
    await Promise.resolve();
    TestBed.tick();
  }

  it('does not show the start page while companies are loading', () => {
    TestBed.tick();
    expect(service.showStartPage()).toBe(false);
    http.expectOne('/api/companies');
  });

  it('shows the start page once an empty list has loaded', async () => {
    await flushCompanies([]);
    expect(service.showStartPage()).toBe(true);
  });

  it('hides the start page when companies exist', async () => {
    await flushCompanies([company]);
    expect(service.showStartPage()).toBe(false);
  });

  it('hides the start page while creating and returns to it on cancel', async () => {
    await flushCompanies([]);

    service.startCreatingCompany();
    expect(service.isCreatingCompany()).toBe(true);
    expect(service.showStartPage()).toBe(false);

    service.stopCreatingCompany();
    expect(service.showStartPage()).toBe(true);
  });

  it('returns to the start page after the last company is deleted', async () => {
    await flushCompanies([company]);
    expect(service.showStartPage()).toBe(false);

    service.deleteCompany('1');
    http.expectOne('/api/companies/1').flush(null);
    TestBed.tick();
    http.expectOne('/api/companies').flush([]);
    await Promise.resolve();
    TestBed.tick();

    expect(service.showStartPage()).toBe(true);
  });

  it('selecting a company leaves creating mode', () => {
    service.startCreatingCompany();
    service.selectCompany(company);
    expect(service.isCreatingCompany()).toBe(false);
    http.match('/api/companies');
  });
});
