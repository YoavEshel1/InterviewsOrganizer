import { Component, computed, inject, Input } from '@angular/core';
import { CdkDrag } from '@angular/cdk/drag-drop';
import { Company } from '../models/company';
import { CompanyService } from '../companyService';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-company-item',
  imports: [CdkDrag, NgIf],
  templateUrl: './company-item.html',
  styleUrl: './company-item.scss',
})
export class CompanyItem {

  @Input() company!: Company;
  private companyService = inject(CompanyService);

  isSelected = computed(() => this.companyService.selectedCompany()?.id === this.company.id);




  editCompany() {
    this.companyService.editCompany(this.company);
  }

}
