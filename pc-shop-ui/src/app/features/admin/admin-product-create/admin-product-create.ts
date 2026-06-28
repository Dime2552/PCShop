import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { CategoryService } from '../../../core/services/category.service';
import { CategoryDto } from '../../../core/models/category.model';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { InputNumberModule } from 'primeng/inputnumber';
import { TextareaModule } from 'primeng/textarea';

@Component({
  selector: 'app-admin-product-create',
  standalone: true,
  imports: [FormsModule, ButtonModule, InputTextModule, SelectModule, InputNumberModule, TextareaModule],
  templateUrl: './admin-product-create.html'
})
export class AdminProductCreate implements OnInit {
  private categoryService = inject(CategoryService);
  private http = inject(HttpClient);
  private messageService = inject(MessageService);
  private router = inject(Router);

  categories = signal<CategoryDto[]>([]);
  isLoading = signal(false);

  // Form Model
  product = {
    name: '',
    categoryId: null as number | null,
    brand: '',
    price: null as number | null,
    stockQuantity: null as number | null,
    description: ''
  };

  // Dynamic Attributes Array
  attributes: { key: string; value: string }[] = [];

  // Files
  selectedFiles: File[] = [];

  ngOnInit() {
    this.categoryService.getCategories().subscribe(res => this.categories.set(res));
  }

  addAttribute() {
    this.attributes.push({ key: '', value: '' });
  }

  removeAttribute(index: number) {
    this.attributes.splice(index, 1);
  }

  onFileSelect(event: any) {
    if (event.target.files) {
      this.selectedFiles = Array.from(event.target.files);
    }
  }

  onSubmit() {
    if (!this.product.categoryId || !this.product.price || !this.product.stockQuantity || this.selectedFiles.length === 0) {
      this.messageService.add({ severity: 'warn', summary: 'Warning', detail: 'Please fill all required fields and select at least 1 image' });
      return;
    }

    this.isLoading.set(true);

    const formData = new FormData();
    formData.append('Name', this.product.name);
    formData.append('CategoryId', this.product.categoryId.toString());
    formData.append('Brand', this.product.brand);
    formData.append('Price', this.product.price.toString());
    formData.append('StockQuantity', this.product.stockQuantity.toString());
    formData.append('Description', this.product.description);

    // Convert attributes array to dictionary JSON
    const attributesDict: Record<string, string> = {};
    this.attributes.forEach(attr => {
      if (attr.key && attr.value) {
        attributesDict[attr.key] = attr.value;
      }
    });
    formData.append('AttributesJson', JSON.stringify(attributesDict));

    // Append files
    this.selectedFiles.forEach(file => {
      formData.append('Images', file, file.name);
    });

    this.http.post(`${environment.apiUrl}/products`, formData).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Product created successfully' });
        this.router.navigate(['/']); // Redirect to catalog
      },
      error: (err) => {
        this.isLoading.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.detail || 'Failed to create product' });
      }
    });
  }
}