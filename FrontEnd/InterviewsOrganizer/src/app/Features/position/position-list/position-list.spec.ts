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
});
