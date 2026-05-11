import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { catchError, finalize, map, tap } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';
import { Company } from './models/company';


@Injectable({
  providedIn: 'root',
})
export class CompanyService {
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


}
