import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CategoryDto } from '../models/category.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CategoryService {
    private http = inject(HttpClient);
    private apiUrl = `${environment.apiUrl}/categories`;

    getCategories(): Observable<CategoryDto[]> {
        return this.http.get<CategoryDto[]>(this.apiUrl);
    }
}