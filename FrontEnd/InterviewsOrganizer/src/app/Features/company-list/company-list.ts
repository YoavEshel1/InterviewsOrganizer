import { Component, inject } from '@angular/core';
import { CdkDragDrop, CdkDropList, moveItemInArray} from '@angular/cdk/drag-drop';
import { CompanyItem } from '../company-item/company-item';
import { CompanyService } from '../companyService';



@Component({
  selector: 'app-company-list',
  imports: [CompanyItem, CdkDropList],
  templateUrl: './company-list.html',
  styleUrl: './company-list.scss' 
})
export class CompanyList {

  private companyService = inject(CompanyService);
  loadingCompanies = this.companyService.loadingCompanies;
  companies = this.companyService.companies;
 

  drop(event: CdkDragDrop<string[]>) {
    moveItemInArray(this.companies(), event.previousIndex, event.currentIndex);
  }

}
