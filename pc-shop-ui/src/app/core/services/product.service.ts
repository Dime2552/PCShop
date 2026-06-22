import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductDto } from '../models/product.model';
import { PaginatedList } from '../models/paginated-list.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private http = inject(HttpClient);

  getProducts(
    categoryId: number,
    pageNumber: number = 1,
    pageSize: number = 10,
    sortBy?: string,
    filters?: Record<string, string>
  ): Observable<PaginatedList<ProductDto>> {
    
    let params = new HttpParams()
      .set('CategoryId', categoryId.toString())
      .set('PageNumber', pageNumber.toString())
      .set('PageSize', pageSize.toString());

    if (sortBy) {
      params = params.set('SortBy', sortBy);
    }

    // .NET wait Dictionary in format Filters[Key]=Value
    if (filters) {
      Object.keys(filters).forEach(key => {
        if (filters[key]) {
          params = params.set(`Filters[${key}]`, filters[key]);
        }
      });
    }

    return this.http.get<PaginatedList<ProductDto>>('https://localhost:7120/api/products', { params });
  }

  getCategoryFilters(categoryId: number): Observable<Record<string, string[]>> {
    return this.http.get<Record<string, string[]>>(`https://localhost:7120/api/products/filters/${categoryId}`);
  }
}