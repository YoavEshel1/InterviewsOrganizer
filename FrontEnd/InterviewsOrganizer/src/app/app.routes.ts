import { Routes } from '@angular/router';
import { CompanyList } from './Features/company-list/company-list';
import { MainArea } from './main-area/main-area';

export const routes: Routes = [

    {        path: '',
        redirectTo: 'companies',
        pathMatch: 'full',
    },
    {
        path: 'companies',
        component: MainArea,
    },
    {
        path: '**',
        redirectTo: 'companies',       
    },

];
