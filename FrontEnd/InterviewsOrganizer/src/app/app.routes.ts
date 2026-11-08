import { inject } from '@angular/core';
import { Routes, Router, CanActivateFn } from '@angular/router';
import { CompanyForm } from './Features/company/company-form/company-form';
import { LandingPage } from './Features/landing-page/landing-page';
import { LoginForm } from './Features/login/login-form/login-form';
import { Register } from './Features/register/register';
import { ForgotPassword } from './Features/forgot-password/forgot-password';
import { ResetPassword } from './Features/reset-password/reset-password';
import { MainArea } from './structure/main-area/main-area';
import { authGuard } from './core/auth/auth.guard';
import { AuthService } from './core/auth/auth.service';

// Inline guard for root URL '/'
export const rootRedirectGuard: CanActivateFn = () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (authService.isAuthenticated()) {
        return router.createUrlTree(['/companies']);
    }

    return router.createUrlTree(['/home']);
};

export const routes: Routes = [
    // Root URL -> Smart redirect
    {
        path: '',
        canActivate: [rootRedirectGuard],
        children: []
    },

    // Public Landing Page
    {
        path: 'home',
        component: LandingPage
    },

    // Protected Main App Area
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

    // Login Page
    {
        path: 'login',
        component: LoginForm,
    },

    // Registration Page
    {
        path: 'signup',
        component: Register,
    },

    // Password reset
    {
        path: 'forgot-password',
        component: ForgotPassword,
    },
    {
        path: 'reset-password',
        component: ResetPassword,
    },

    // Wildcard fallback
    {
        path: '**',
        redirectTo: '',
    }
];