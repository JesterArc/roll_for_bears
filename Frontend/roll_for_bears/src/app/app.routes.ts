import { Routes } from '@angular/router';
import { Login } from './pages/login/login'
import { Register } from './pages/register/register';
import { ForgotPassword } from './pages/forgot-password/forgot-password';
import { Homepage } from './pages/homepage/homepage';
import { YourSessions } from './pages/your-sessions/your-sessions';
import { Sessions } from './pages/sessions/sessions';
import { CreateSession } from './pages/create-session/create-session';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
  {
    path: "login",
    component: Login
  },
  {
    path: 'register',
    component: Register
  },
  {
    path: 'forgot-password',
    component: ForgotPassword
  },
  {
    path: 'home',
    component: Homepage
  },
  {
    path: 'your-sessions',
    component: YourSessions
  },
  {
    path: 'sessions',
    component: Sessions
  },
  {
    path: 'create-session',
    component: CreateSession
  }
];
