import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductDto, UpdateProductStockAndPriceRequest } from '../models/product.model';
import { ReviewDto } from '../models/review.model';
import { PaginatedList } from '../models/paginated-list.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/products`;

  getProducts(
    categoryId?: number,
    pageNumber: number = 1,
    pageSize: number = 10,
    sortBy?: string,
    filters?: Record<string, string>
  ): Observable<PaginatedList<ProductDto>> {
    
    let params = new HttpParams()
      .set('PageNumber', pageNumber.toString())
      .set('PageSize', pageSize.toString());

    if (categoryId !== undefined && categoryId !== null && categoryId > 0) {
      params = params.set('CategoryId', categoryId.toString());
    }

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

    return this.http.get<PaginatedList<ProductDto>>(this.apiUrl, { params });
  }

  getProductById(id: string): Observable<ProductDto> {
    return this.http.get<ProductDto>(`${this.apiUrl}/${id}`);
  }

  getReviews(productId: string): Observable<ReviewDto[]> {
    return this.http.get<ReviewDto[]>(`${this.apiUrl}/${productId}/reviews`);
  }

  addReview(productId: string, rating: number, comment: string): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/${productId}/reviews`, { rating, comment });
  }

  getCategoryFilters(categoryId: number): Observable<Record<string, string[]>> {
    return this.http.get<Record<string, string[]>>(`${this.apiUrl}/filters/${categoryId}`);
  }

  updateProductStockAndPrice(id: string, request: UpdateProductStockAndPriceRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/stock-price`, request);
  }

  deleteProduct(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}