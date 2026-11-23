import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting, TestRequest } from '@angular/common/http/testing';
import { By } from '@angular/platform-browser';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PositionForm } from '../position-form/position-form';
import { PositionArea } from './position-area';

describe('PositionArea', () => {
  let http: HttpTestingController;

  async function createComponent(flushInitialRequest = true): Promise<{
    fixture: ComponentFixture<PositionArea>;
    initialRequest: TestRequest;
  }> {
    TestBed.configureTestingModule({
      imports: [PositionArea],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    const fixture = TestBed.createComponent(PositionArea);
    fixture.componentRef.setInput('companyId', 'c1');
    http = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
    const initialRequest = http.expectOne('/api/companies/c1/positions');
    if (flushInitialRequest) {
      initialRequest.flush([]);
      await fixture.whenStable();
    }
    return { fixture, initialRequest };
  }

  it('renders the form on request and the list with the company ID', async () => {
    const { fixture } = await createComponent();
    const button: HTMLButtonElement = fixture.nativeElement.querySelector('button');

    expect(fixture.nativeElement.querySelector('app-position-list')).not.toBeNull();
    button.click();
    fixture.detectChanges();

    const form = fixture.debugElement.query(By.directive(PositionForm)).componentInstance as PositionForm;
    expect(form.companyId()).toBe('c1');
    expect(fixture.nativeElement.querySelector('app-position-list')).not.toBeNull();
  });

  it('reloads the list and emits the created position after save', async () => {
    const { fixture } = await createComponent();
    fixture.nativeElement.querySelector('button').click();
    fixture.detectChanges();

    const form = fixture.debugElement.query(By.directive(PositionForm)).componentInstance as PositionForm;
    const position = {
      id: 'p1',
      title: 'Engineer',
      companyId: 'c1',
      applyDate: null,
      status: 0,
      notes: '',
    };
    const saved: unknown[] = [];
    fixture.componentInstance.saved.subscribe((value) => saved.push(value));

    form['form'].patchValue({ title: 'Engineer' });
    form['onSubmit']();
    http.expectOne('/api/companies/c1/positions').flush(position);
    fixture.detectChanges();

    const reload = http.expectOne('/api/companies/c1/positions');
    expect(reload.request.method).toBe('GET');
    reload.flush([position]);
    expect(fixture.nativeElement.querySelector('app-position-form')).toBeNull();
    expect(saved).toEqual([position]);
  });

  it('reloads after an in-flight initial request completes', async () => {
    const { fixture, initialRequest } = await createComponent(false);
    fixture.nativeElement.querySelector('button').click();
    fixture.detectChanges();

    const form = fixture.debugElement.query(By.directive(PositionForm)).componentInstance as PositionForm;
    const position = {
      id: 'p1',
      title: 'Engineer',
      companyId: 'c1',
      applyDate: null,
      status: 0,
      notes: '',
    };
    form['form'].patchValue({ title: 'Engineer' });
    form['onSubmit']();
    http
      .expectOne(
        (request) => request.method === 'POST' && request.url === '/api/companies/c1/positions',
      )
      .flush(position);

    fixture.detectChanges();
    TestBed.tick();
    expect(initialRequest.cancelled).toBe(true);

    const reload = http.expectOne(
      (request) => request.method === 'GET' && request.url === '/api/companies/c1/positions',
    );
    expect(reload.request.method).toBe('GET');
    reload.flush([position]);
  });
});
