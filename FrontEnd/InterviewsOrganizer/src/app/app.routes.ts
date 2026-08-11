import { Routes } from '@angular/router';
import { CompanyList } from './Features/company/company-list/company-list';
import { MainArea } from './structure/main-area/main-area';
import { LoginForm } from './Features/login/login-form/login-form';

export const routes: Routes = [

    {
        path: '',
        redirectTo: 'companies',
        pathMatch: 'full',
    },
    {
        path: 'companies',
        component: MainArea,
    },
    {
        path: 'login',
        component: LoginForm,
    },
    {
        path: '**',
        redirectTo: 'companies',
    },

];
