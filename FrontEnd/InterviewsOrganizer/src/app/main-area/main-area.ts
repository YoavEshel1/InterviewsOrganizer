import { Component } from '@angular/core';
import { CompanyList } from "../Features/company-list/company-list";
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-main-area',
  imports: [CompanyList, RouterOutlet],
  templateUrl: './main-area.html',
  styleUrl: './main-area.scss',
})
export class MainArea {

}
