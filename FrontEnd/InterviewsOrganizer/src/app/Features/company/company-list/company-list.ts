import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { CdkDragDrop, CdkDropList, moveItemInArray} from '@angular/cdk/drag-drop';
import { MatButtonModule } from '@angular/material/button';
import { CompanyItem } from '../company-item/company-item';
import { CompanyService } from '../companyService';



@Component({
  selector: 'app-company-list',
  imports: [CompanyItem, CdkDropList, MatButtonModule],
  templateUrl: './company-list.html',
  styleUrl: './company-list.scss' ,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CompanyList {

  private companyService = inject(CompanyService);
  loadingCompanies = this.companyService.loadingCompanies;
  companies = this.companyService.companies;
  isCreatingCompany = this.companyService.isCreatingCompany;

  addCompany() {
    this.companyService.startCreatingCompany();
  }
 

  drop(event: CdkDragDrop<string[]>) {
    moveItemInArray(this.companies(), event.previousIndex, event.currentIndex);
  }

}
