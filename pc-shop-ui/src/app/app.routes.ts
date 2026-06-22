import { Routes } from '@angular/router';
import { Catalog } from './features/catalog/catalog';

export const routes: Routes = [
  { path: '', component: Catalog },
  { path: '**', redirectTo: '' }
];