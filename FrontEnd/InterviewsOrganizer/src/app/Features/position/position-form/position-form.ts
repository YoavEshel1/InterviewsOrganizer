import { ChangeDetectionStrategy, Component, DestroyRef, inject, input, output, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { provideNativeDateAdapter } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule, MatIconRegistry } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { DomSanitizer } from '@angular/platform-browser';
import { finalize } from 'rxjs';
import { getServerErrorMessage } from '../../../core/auth/server-error';
import { Position, POSITION_STATUS, POSITION_STATUS_OPTIONS, PositionStatus } from '../../models/position';
import { START_PAGE_ICONS } from '../../start-page/start-page-icons';
import { PositionService } from '../position.service';

/** Local yyyy-MM-dd, so the chosen day is not shifted by the timezone. */
function toDateOnly(date: Date): string {
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${date.getFullYear()}-${month}-${day}`;
}

@Component({
  selector: 'app-position-form',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './position-form.html',
  styleUrl: './position-form.scss',
})
export class PositionForm {
  private readonly positionService = inject(PositionService);
  private readonly destroyRef = inject(DestroyRef);

  readonly companyId = input.required<string>();
  readonly saved = output<Position>();
  readonly cancelled = output<void>();

  protected readonly statusOptions = POSITION_STATUS_OPTIONS;
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');

  protected readonly form = inject(NonNullableFormBuilder).group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    applyDate: new FormControl<Date | null>(null),
    status: [POSITION_STATUS.Applied as PositionStatus],
    notes: ['', Validators.maxLength(2000)],
  });

  constructor() {
    const registry = inject(MatIconRegistry);
    const sanitizer = inject(DomSanitizer);
    for (const [name, markup] of Object.entries(START_PAGE_ICONS)) {
      registry.addSvgIconLiteral(name, sanitizer.bypassSecurityTrustHtml(markup));
    }
  }

  protected onSubmit(): void {
    if (this.saving()) {
      return;
    }
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { title, applyDate, status, notes } = this.form.getRawValue();
    this.errorMessage.set('');
    this.saving.set(true);

    this.positionService
      .create(this.companyId(), {
        title: title.trim(),
        applyDate: applyDate ? toDateOnly(applyDate) : null,
        status,
        notes,
      })
      .pipe(
        finalize(() => this.saving.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (position) => {
          this.form.reset();
          this.saved.emit(position);
        },
        error: (error) =>
          this.errorMessage.set(getServerErrorMessage(error, 'Could not save the position. Please try again.')),
      });
  }
}
