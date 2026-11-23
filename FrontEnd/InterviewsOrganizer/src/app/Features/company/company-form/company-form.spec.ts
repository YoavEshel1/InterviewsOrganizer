import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed, ComponentFixture } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { By } from '@angular/platform-browser';
import { CompanyService } from '../companyService';
import { PositionArea } from '../../position/position-area/position-area';
import { CompanyForm } from './company-form';

describe('CompanyForm', () => {
  let service: CompanyService;
  let http: HttpTestingController;

  function createComponent(): CompanyForm {
    TestBed.configureTestingModule({
      imports: [CompanyForm],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(CompanyService);
    http = TestBed.inject(HttpTestingController);
    const fixture = TestBed.createComponent(CompanyForm);
    fixture.detectChanges();
    return fixture.componentInstance;
  }

  it('should create', () => {
    expect(createComponent()).toBeTruthy();
  });

  it('starts blank and creates a company with POST when creating', () => {
    const component = createComponent();
    service.startCreatingCompany();
    TestBed.tick();

    expect(component.form.getRawValue().name).toBe('');

    component.form.patchValue({ name: 'Acme' });
    component.saveChanges();

    const req = http.expectOne((r) => r.method === 'POST' && r.url === '/api/companies');
    expect(req.request.body.id).toBeUndefined();
    expect(req.request.body.name).toBe('Acme');
    http.match('/api/companies');
  });

  it('does not save an unnamed company', () => {
    const component = createComponent();
    service.startCreatingCompany();
    TestBed.tick();

    component.saveChanges();

    http.expectNone((r) => r.method === 'POST');
    http.match('/api/companies');
  });

  it('cancel leaves creating mode', () => {
    const component = createComponent();
    service.startCreatingCompany();

    component.cancel();

    expect(service.isCreatingCompany()).toBe(false);
    http.match('/api/companies');
  });

  describe('positions', () => {
    const company = { id: 'c1', name: 'Acme', positions: [] };
    const position = { id: 'p1', title: 'Engineer', companyId: 'c1', applyDate: '2026-11-11', status: 0 as const };

    function createFixture(): ComponentFixture<CompanyForm> {
      TestBed.configureTestingModule({
        imports: [CompanyForm],
        providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
      });
      service = TestBed.inject(CompanyService);
      http = TestBed.inject(HttpTestingController);
      const fixture = TestBed.createComponent(CompanyForm);
      fixture.detectChanges();
      return fixture;
    }

    it('passes the selected company ID to PositionArea', () => {
      const fixture = createFixture();
      service.selectCompany(company);
      fixture.detectChanges();

      const area = fixture.debugElement.query(By.directive(PositionArea)).componentInstance as PositionArea;
      expect(area.companyId()).toBe('c1');
      http.match('/api/companies');
      http.match('/api/companies/c1/positions').forEach((request) => request.flush([]));
    });

    it('updates the selected company when PositionArea emits a saved position', () => {
      const fixture = createFixture();
      service.selectCompany(company);
      fixture.detectChanges();

      const area = fixture.debugElement.query(By.directive(PositionArea)).componentInstance as PositionArea;
      area.saved.emit(position);
      expect(service.selectedCompany()?.positions).toEqual([position]);
      http.match('/api/companies');
      http.match('/api/companies/c1/positions').forEach((request) => request.flush([]));
    });
  });
});
