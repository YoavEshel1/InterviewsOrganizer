import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginArea } from './login-area';

describe('LoginArea', () => {
  let component: LoginArea;
  let fixture: ComponentFixture<LoginArea>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoginArea]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LoginArea);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
