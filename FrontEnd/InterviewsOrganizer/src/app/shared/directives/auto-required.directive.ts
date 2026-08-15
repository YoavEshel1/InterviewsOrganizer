import { Directive, ElementRef, inject, OnInit } from '@angular/core';
import { NgControl, Validators } from '@angular/forms';

@Directive({
  selector: '[formControlName]',
  standalone: true,
})
export class AutoRequiredDirective implements OnInit {
  private ngControl = inject(NgControl);
  private el = inject(ElementRef<HTMLElement>);

  ngOnInit() {
    if (this.ngControl.control?.hasValidator(Validators.required)) {
      this.el.nativeElement.setAttribute('required', '');
    }
  }
}
