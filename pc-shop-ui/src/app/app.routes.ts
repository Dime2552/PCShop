import { Routes } from '@angular/router';
import { CatalogComponent } from './features/catalog/catalog';
import { CartComponent } from './features/cart/cart';

export const routes: Routes = [
  { path: '', component: CatalogComponent },
  { path: 'cart', component: CartComponent },
  { path: '**', redirectTo: '' }
];