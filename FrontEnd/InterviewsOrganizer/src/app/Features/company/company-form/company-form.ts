import { Component, effect, inject } from '@angular/core';
import { Company } from '../../models/company';
import { CompanyService } from '../companyService';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule, MatIconRegistry } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { DomSanitizer } from '@angular/platform-browser';
import { Interview } from '../../models/interview';
import { Position } from '../../models/position';
import { ActivatedRoute } from '@angular/router';
import { START_PAGE_ICONS } from '../../start-page/start-page-icons';
import { PositionArea } from '../../position/position-area/position-area';

@Component({
  selector: 'app-company-form',
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    PositionArea,
  ],
  templateUrl: './company-form.html',
  styleUrl: './company-form.scss',
})
export class CompanyForm {

  companyService = inject(CompanyService);
  isCreating = this.companyService.isCreatingCompany;
  private fb = inject(FormBuilder);

  form = this.fb.group({
    name: ['', Validators.required],
    companyInfo: [''],
    logoUrl: [''],
    positions: this.fb.array([]) // This will hold the positions form groups  
  });

  company = this.companyService.selectedCompany;
  get positions(): FormArray {
    return this.form.get('positions') as FormArray;
  }

  getInterviews(positionIndex: number): FormArray {
    return this.positions
      .at(positionIndex)
      .get('interviews') as FormArray;
  }

  private route = inject(ActivatedRoute);

  constructor() {
    const registry = inject(MatIconRegistry);
    const sanitizer = inject(DomSanitizer);
    for (const [name, markup] of Object.entries(START_PAGE_ICONS)) {
      registry.addSvgIconLiteral(name, sanitizer.bypassSecurityTrustHtml(markup));
    }
    effect(() => {
      const id = this.route.snapshot.paramMap.get('id');
      const company = this.companyService.companies().find(c => c.id === id);
      if (company) {
        this.companyService.editCompany(company);
      }
    });

    effect(() => {
      const selectedCompany = this.companyService.selectedCompany();
      if (selectedCompany) {
        this.rebuildForm(selectedCompany);
      }
    });

    effect(() => {
      if (this.companyService.isCreatingCompany()) {
        this.resetForm();
      }
    });

  }

  private resetForm() {
    this.form.reset({ name: '', companyInfo: '', logoUrl: '' });
    this.positions.clear();
  }

  rebuildForm(selectedCompany: Company) {
    this.form.patchValue({
      name: selectedCompany.name,
      companyInfo: selectedCompany.companyInfo,
      logoUrl: selectedCompany.logoUrl
    });

    // 2. Rebuild the positions FormArray
    this.positions.clear();
    if (selectedCompany.positions) {
      selectedCompany.positions.forEach(p => {
        this.positions.push(this.makePositionGroup(p));
      });
    }
  }

  /** Was a top-level field touched and is invalid? */
  isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl?.invalid && ctrl.touched);
  }

  private makePositionGroup(p: Position): FormGroup {
    const group = this.fb.group({
      id: [p.id],
      title: [p.title, Validators.required],
      applyDate: [p.applyDate, Validators.required],
      status: [p.status, Validators.required],
      notes: [p.notes],
      interviews: this.fb.array(
        (p.interviews ?? []).map(i => this.makeInterviewGroup(i))
      ),
    });

    return group;
  }

  private makeInterviewGroup(i: Interview): FormGroup {
    return this.fb.group({
      id: [i.id],
      date: [i.date, Validators.required],
      feeling: [i.feeling, Validators.required],
      interviewer: [i.interviewer],
      notes: [i.notes],
    });
  }

  onPositionSaved(position: Position): void {
    this.companyService.addPositionToCompany(position.companyId || this.company()!.id, position);
  }

  removePosition(index: number): void {
    this.positions.removeAt(index);
  }

  addInterview(positionIndex: number): void {
    //TODO: call the server to create the interview and get the id and date back
    // this.getInterviews(positionIndex).push(this.makeInterviewGroup({        
    //   feeling:         2,
    //   interviewer: '',
    //   notes:           '',
    // }));
  }

  removeInterview(positionIndex: number, interviewIndex: number): void {
    this.getInterviews(positionIndex).removeAt(interviewIndex);
  }

  saveChanges() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.companyService.save({
      id: this.companyService.selectedCompany()?.id,
      ...this.form.getRawValue(),
    } as Company);
    this.form.markAsPristine();
  }

  cancel(): void {
    const selectedCompany = this.company();

    if (selectedCompany) {
      this.rebuildForm(selectedCompany);
      this.form.markAsPristine();
      this.form.markAsUntouched();
      return;
    }

    // Canceling a new company form
    this.resetForm();
    this.companyService.clearSelection();
    this.companyService.stopCreatingCompany();
  }

}
