import { Routes } from '@angular/router';
import { authGuard, guestGuard } from '../core/guards/auth.guard';

export const routes: Routes = [
  { 
    path: '', 
    loadComponent: () => import('../features/home/home').then(m => m.HomeComponent),
    title: 'Lunex - Find Your Match'
  },
  { 
    path: 'login', 
    loadComponent: () => import('../features/auth/login/login').then(m => m.LoginComponent),
    canActivate: [guestGuard],
    title: 'Sign In - Lunex'
  },
  { 
    path: 'register', 
    loadComponent: () => import('../features/auth/register/register').then(m => m.RegisterComponent),
    canActivate: [guestGuard],
    title: 'Create Account - Lunex'
  },
  { 
    path: 'members', 
    loadComponent: () => import('../features/members/member-list/member-list').then(m => m.MemberListComponent),
    canActivate: [authGuard],
    title: 'Discover Members - Lunex'
  },
  { 
    path: 'members/:id', 
    loadComponent: () => import('../features/members/member-detailed/member-detailed').then(m => m.MemberDetailedComponent),
    canActivate: [authGuard],
    title: 'Member Profile - Lunex'
  },
  { 
    path: 'lists', 
    loadComponent: () => import('../features/lists/lists').then(m => m.ListsComponent),
    canActivate: [authGuard],
    title: 'My Lists - Lunex'
  },
  { 
    path: 'messages', 
    loadComponent: () => import('../features/messages/messages').then(m => m.MessagesComponent),
    canActivate: [authGuard],
    title: 'Messages - Lunex'
  },
  { 
    path: 'forgot-password', 
    loadComponent: () => import('../features/auth/forgot-password/forgot-password').then(m => m.ForgotPasswordComponent),
    canActivate: [guestGuard],
    title: 'Reset Password - Lunex'
  },
  { path: '**', redirectTo: '' },
];