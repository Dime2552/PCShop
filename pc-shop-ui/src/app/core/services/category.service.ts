import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CategoryDto } from '../models/category.model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
    private http = inject(HttpClient);

    getCategories(): Observable<CategoryDto[]> {
        return this.http.get<CategoryDto[]>('https://localhost:7120/api/categories');
    }
}