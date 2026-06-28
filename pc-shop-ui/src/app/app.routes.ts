import { Routes } from '@angular/router';
import { CatalogComponent } from './features/catalog/catalog';
import { CartComponent } from './features/cart/cart';
import { AuthComponent } from './features/auth/auth';
import { adminGuard } from './core/guards/admin.guard';
import { AdminProductCreate } from './features/admin/admin-product-create/admin-product-create';

export const routes: Routes = [
  { path: '', component: CatalogComponent },
  { path: 'cart', component: CartComponent },
  { path: 'auth', component: AuthComponent },
  { path: 'admin/product/new', component: AdminProductCreate, canActivate: [adminGuard] },
  { path: '**', redirectTo: '' }
];