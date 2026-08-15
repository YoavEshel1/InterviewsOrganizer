import { HttpClient, httpResource } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import {  finalize, tap } from 'rxjs';
import { Company } from '../models/company';


@Injectable({
  providedIn: 'root',
})
export class CompanyService {

  private  http =inject(HttpClient);

  private companiesUrl = '/api/companies';
  readonly companiesResource = httpResource<Company[]>(() =>
    ({ url: this.companiesUrl }),
    { defaultValue: [] as Company[] }
  );

  //public
  readonly companies = this.companiesResource.value;
  readonly loadingCompanies = this.companiesResource.isLoading;

  //selected company
  private selectedCompanySignal = signal<Company | null>(null);
  readonly selectedCompany = this.selectedCompanySignal.asReadonly();



 save(company: Company) {
    const isEdit = Boolean(company.id);
    const request$ = isEdit
      ? this.http.put<Company>(`${this.companiesUrl}/${company.id}`, company)
      : this.http.post<Company>(this.companiesUrl, company);

    request$.pipe(
      finalize(() => {
        //automatically reload the companies list after saving
        this.companiesResource.reload();
        this.clearSelection();
      })
    ).subscribe();
  }

  //delete a company by id
  deleteCompany(id: string) {
    this.http.delete(`${this.companiesUrl}/${id}`).pipe(
      finalize(() => {
        this.companiesResource.reload();
        // check if the deleted company is the selected one, and if so, clear the selection
        const selectedCompany = this.selectedCompany();
        if (selectedCompany && selectedCompany.id === id) {
          this.clearSelection();
        }
      })
    ).subscribe();
  }

  //state management

  selectCompany(company: Company) {
    this.selectedCompanySignal.set(company);
  }

  editCompany(company: Company) {
    this.selectedCompanySignal.set(company);
  }

  clearSelection() {
    this.selectedCompanySignal.set(null);
  }
}
