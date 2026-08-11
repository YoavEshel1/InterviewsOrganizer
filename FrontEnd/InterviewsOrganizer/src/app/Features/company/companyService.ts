import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, Signal, signal } from '@angular/core';
import { catchError, finalize, map, tap } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';
import { Company } from '../models/company';


@Injectable({
  providedIn: 'root',
})
export class CompanyService {
  clearSelection() {
    throw new Error('Method not implemented.');
  }
  save(arg0: Company) {
    throw new Error('Method not implemented.');
  }


  private companiesUrl = '/api/companies';
  private http = inject(HttpClient);
  private loadingCompaniesSignal = signal<boolean>(false);
  readonly loadingCompanies = this.loadingCompaniesSignal.asReadonly();

  

  private companies$ = this.http.get<Company[]>(this.companiesUrl)
    .pipe(
      tap(() => this.loadingCompaniesSignal.set(true)),     
      map((response: any) => response as Company[]),
      finalize(() => this.loadingCompaniesSignal.set(false)),
      catchError((error) => {
        // Handle the error appropriately, e.g., log it or show a notification
        console.error('Error fetching companies:', error);
        return [];
      })    
    );

    companies = toSignal(this.companies$, { initialValue: [] });
    //default to the first company or null if the list is empty
    private selectedCompanySignal = signal<Company | null>(this.companies()[0] || null);
    selectedCompany = this.selectedCompanySignal.asReadonly();

  editCompany(company: Company) {
    this.selectedCompanySignal.set(company);
  }
}
