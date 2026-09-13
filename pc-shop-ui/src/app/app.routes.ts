import { Routes } from '@angular/router';
import { CatalogComponent } from './features/catalog/catalog';
import { CartComponent } from './features/cart/cart';
import { AuthComponent } from './features/auth/auth';
import { adminGuard } from './core/guards/admin.guard';
import { AdminLayoutComponent } from './features/admin/admin-layout/admin-layout';
import { CheckoutComponent } from './features/checkout/checkout';
import { CheckoutSuccessComponent } from './features/checkout/checkout-success';
import { CheckoutCancelComponent } from './features/checkout/checkout-cancel';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', component: CatalogComponent },
  {
    path: 'product/:id',
    loadComponent: () => import('./features/product-details/product-details').then(m => m.ProductDetailsComponent)
  },
  { path: 'cart', component: CartComponent },
  { path: 'checkout', component: CheckoutComponent, canActivate: [authGuard] },
  { path: 'checkout/success', component: CheckoutSuccessComponent },
  { path: 'checkout/cancel', component: CheckoutCancelComponent },
  {
    path: 'profile/orders',
    loadComponent: () => import('./features/profile/my-orders/my-orders').then(m => m.MyOrdersComponent),
    canActivate: [authGuard]
  },
  { path: 'auth', component: AuthComponent },
  {
    path: 'admin',
    component: AdminLayoutComponent,
    canActivate: [adminGuard],
    children: [
      { path: 'orders', loadComponent: () => import('./features/admin/admin-orders/admin-order-list').then(m => m.AdminOrderListComponent) },
      { path: 'product/new', loadComponent: () => import('./features/admin/admin-product-create/admin-product-create').then(m => m.AdminProductCreate) },
      { path: '', redirectTo: 'orders', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: '' }
];
