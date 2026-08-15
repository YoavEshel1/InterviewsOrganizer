import { Routes } from '@angular/router';
import { CompanyList } from './Features/company/company-list/company-list';
import { MainArea } from './structure/main-area/main-area';
import { LoginForm } from './Features/login/login-form/login-form';
import { authGuard } from './core/auth/auth.guard';
import { CompanyForm } from './Features/company/company-form/company-form';

export const routes: Routes = [

    {
        path: '',
        redirectTo: 'companies',
        pathMatch: 'full',
    },
    {
        path: 'companies',
        component: MainArea,
        canActivate: [authGuard],
        children: [
            {
                path: ':id',
                component: CompanyForm,
            },
        ],
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
