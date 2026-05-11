import { Component, Input } from '@angular/core';
import { CdkDrag } from '@angular/cdk/drag-drop';
import { Company } from '../models/company';

@Component({
  selector: 'app-company-item',
  imports: [ CdkDrag],
  templateUrl: './company-item.html',
  styleUrl: './company-item.scss',
})
export class CompanyItem {

  @Input() company!: Company;

}
  