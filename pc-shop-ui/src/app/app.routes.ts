import { Routes } from '@angular/router';
import { CatalogComponent } from './features/catalog/catalog';
import { CartComponent } from './features/cart/cart';
import { AuthComponent } from './features/auth/auth';

export const routes: Routes = [
  { path: '', component: CatalogComponent },
  { path: 'cart', component: CartComponent },
  { path: 'auth', component: AuthComponent },
  { path: '**', redirectTo: '' }
];