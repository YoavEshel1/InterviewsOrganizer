import { Component, computed, inject, input } from '@angular/core';
import { CdkDrag, CdkDragHandle } from '@angular/cdk/drag-drop';
import { Company } from '../../models/company';
import { CompanyService } from '../companyService';
import { NgIf } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-company-item',
  imports: [CdkDrag, CdkDragHandle, NgIf],
  templateUrl: './company-item.html',
  styleUrl: './company-item.scss',
})
export class CompanyItem {

  company =input.required<Company>();
  private companyService = inject(CompanyService);
  private router = inject(Router);

  isSelected = computed(() => this.companyService.selectedCompany()?.id === this.company().id);

  selectCompany() {
    this.router.navigate(['/companies', this.company().id]);
    this.companyService.selectCompany(this.company());
  }

  editCompany() {
    this.companyService.editCompany(this.company());
  }

}
