import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule, MatIconRegistry } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';
import { CompanyService } from '../company/companyService';
import { START_PAGE_ICONS } from './start-page-icons';

interface HierarchyStep {
  icon: string;
  title: string;
  description: string;
}

@Component({
  selector: 'app-start-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatCardModule, MatButtonModule, MatIconModule],
  templateUrl: './start-page.html',
  styleUrl: './start-page.scss',
})
export class StartPage {
  private readonly companyService = inject(CompanyService);

  protected readonly steps: readonly HierarchyStep[] = [
    {
      icon: 'sp-company',
      title: 'Company',
      description: 'Start by adding a company you are interested in or have applied to.',
    },
    {
      icon: 'sp-position',
      title: 'Positions',
      description: 'Inside each company, add the positions you are applying for.',
    },
    {
      icon: 'sp-interview',
      title: 'Interviews',
      description: 'Inside each position, log every interview: date, interviewer, notes and how it felt.',
    },
  ];

  constructor() {
    const registry = inject(MatIconRegistry);
    const sanitizer = inject(DomSanitizer);
    for (const [name, markup] of Object.entries(START_PAGE_ICONS)) {
      registry.addSvgIconLiteral(name, sanitizer.bypassSecurityTrustHtml(markup));
    }
  }

  protected addFirstCompany(): void {
    this.companyService.startCreatingCompany();
  }
}
