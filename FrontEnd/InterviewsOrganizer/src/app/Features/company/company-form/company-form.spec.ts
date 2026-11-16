import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed, ComponentFixture } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { CompanyService } from '../companyService';
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

    function addPositionButton(fixture: ComponentFixture<CompanyForm>): HTMLButtonElement {
      const buttons: HTMLButtonElement[] = Array.from(fixture.nativeElement.querySelectorAll('.positions button'));
      return buttons.find((b) => b.textContent?.includes('Add position'))!;
    }

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

    it('keeps Add position disabled until a saved company is selected', () => {
      const fixture = createFixture();
      expect(addPositionButton(fixture).disabled).toBe(true);

      service.selectCompany(company);
      fixture.detectChanges();
      expect(addPositionButton(fixture).disabled).toBe(false);
      http.match('/api/companies');
    });

    it('shows the position form under the button when clicked', () => {
      const fixture = createFixture();
      service.selectCompany(company);
      fixture.detectChanges();
      expect(fixture.nativeElement.querySelector('app-position-form')).toBeNull();

      addPositionButton(fixture).click();
      fixture.detectChanges();

      expect(fixture.nativeElement.querySelector('app-position-form')).not.toBeNull();
      http.match('/api/companies');
    });

    it('lists the saved position and closes the form', () => {
      const fixture = createFixture();
      service.selectCompany(company);
      fixture.componentInstance.addPosition();

      fixture.componentInstance.onPositionSaved(position);
      fixture.detectChanges();

      expect(fixture.componentInstance.showPositionForm()).toBe(false);
      expect(fixture.nativeElement.querySelector('.position-item')?.textContent).toContain('Engineer');
      expect(service.selectedCompany()?.positions).toEqual([position]);
      http.match('/api/companies');
    });
  });
});
