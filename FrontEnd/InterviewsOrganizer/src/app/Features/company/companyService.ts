import { HttpClient, httpResource } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import {  finalize, tap } from 'rxjs';
import { Company } from '../models/company';
import { Position } from '../models/position';


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
  readonly companiesError = this.companiesResource.error;

  //selected company
  private selectedCompanySignal = signal<Company | null>(null);
  readonly selectedCompany = this.selectedCompanySignal.asReadonly();

  //creating a new company (blank form)
  private creatingCompanySignal = signal(false);
  readonly isCreatingCompany = this.creatingCompanySignal.asReadonly();

  /** The Start Page replaces the main area only once the list has really loaded empty. */
  readonly showStartPage = computed(
    () =>
      !this.loadingCompanies() &&
      !this.companiesError() &&
      this.companies().length === 0 &&
      !this.isCreatingCompany(),
  );



 save(company: Company) {
    const isEdit = Boolean(company.id);
    const request$ = isEdit
      ? this.http.put<Company>(`${this.companiesUrl}`, company)
      : this.http.post<Company>(this.companiesUrl, company);

    request$.pipe(
      finalize(() => {
        //automatically reload the companies list after saving
        this.companiesResource.reload();
        this.clearSelection();
        this.stopCreatingCompany();
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
    this.creatingCompanySignal.set(false);
    this.selectedCompanySignal.set(company);
  }

  editCompany(company: Company) {
    this.creatingCompanySignal.set(false);
    this.selectedCompanySignal.set(company);
  }

  clearSelection() {
    this.selectedCompanySignal.set(null);
  }

  startCreatingCompany() {
    this.selectedCompanySignal.set(null);
    this.creatingCompanySignal.set(true);
  }

  /** Adds a freshly created position to the cached list and the selected company without refetching. */
  addPositionToCompany(companyId: string, position: Position) {
    //Defines a helper function that takes a Company and returns a Company 
    // for the mapping operation, adding the new position if the company ID matches.
    const withPosition = (c: Company): Company =>
      c.id === companyId ? { ...c, positions: [...(c.positions ?? []), position] } : c;

    // Update the cached list of companies with the new position
    this.companiesResource.value.update((list) => list.map(withPosition));
    const selected = this.selectedCompany();
    if (selected) {
      this.selectedCompanySignal.set(withPosition(selected));
    }
  }

  stopCreatingCompany() {
    this.creatingCompanySignal.set(false);
  }
}
