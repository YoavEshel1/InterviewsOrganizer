import { ChangeDetectionStrategy, Component, effect, inject } from '@angular/core';
import { CompanyList } from "../../Features/company/company-list/company-list";
import { CompanyForm } from '../../Features/company/company-form/company-form';
import { CompanyService } from '../../Features/company/companyService';
import { StartPage } from '../../Features/start-page/start-page';
import { Router, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-main-area',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CompanyList, CompanyForm, RouterOutlet, StartPage],
  templateUrl: './main-area.html',
  styleUrl: './main-area.scss',
})
export class MainArea {
  private readonly companyService = inject(CompanyService);
  private readonly router = inject(Router);

  protected readonly showStartPage = this.companyService.showStartPage;
  protected readonly isCreatingCompany = this.companyService.isCreatingCompany;

  constructor() {
    // Leave any /companies/:id child route so it cannot re-select a company over the blank form.
    effect(() => {
      if (this.isCreatingCompany()) {
        void this.router.navigate(['/companies']);
      }
    });
  }
}
