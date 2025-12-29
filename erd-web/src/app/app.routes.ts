import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login';
import { AssetsComponent } from './pages/assets/assets';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'assets', component: AssetsComponent },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
];