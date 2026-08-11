import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CompanyItem } from './company-item';

describe('CompanyItem', () => {
  let component: CompanyItem;
  let fixture: ComponentFixture<CompanyItem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CompanyItem]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CompanyItem);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
