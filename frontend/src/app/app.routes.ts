import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { AuthLayoutComponent } from './layouts/auth-layout/auth-layout.component';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'login' },

  {
    path: '',
    component: AuthLayoutComponent,
    children: [
      {
        path: 'login',
        loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
      },
      {
        path: 'register',
        loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
      }
    ]
  },

  {
    path: '',
    component: MainLayoutComponent,
    // Guarding the layout covers every child, so a new page cannot be added unprotected.
    canActivate: [authGuard],
    children: [
      {
        path: 'contacts',
        loadComponent: () =>
          import('./features/contacts/contact-list/contact-list.component').then(m => m.ContactListComponent)
      },
      {
        path: 'settings',
        loadComponent: () =>
          import('./features/lookups/lookup-management/lookup-management.component').then(
            m => m.LookupManagementComponent
          )
      }
    ]
  },

  { path: '**', redirectTo: 'login' }
];
