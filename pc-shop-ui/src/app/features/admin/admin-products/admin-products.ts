import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { MessageService } from 'primeng/api';

import { ProductService } from '../../../core/services/product.service';
import { CategoryService } from '../../../core/services/category.service';
import { ProductDto, UpdateProductStockAndPriceRequest } from '../../../core/models/product.model';
import { CategoryDto } from '../../../core/models/category.model';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    DatePipe,
    CurrencyPipe,
    TableModule,
    TagModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
    InputNumberModule,
    SelectModule
  ],
  templateUrl: './admin-products.html'
})
export class AdminProductsComponent implements OnInit {
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private messageService = inject(MessageService);

  // Data Signals
  products = signal<ProductDto[]>([]);
  categories = signal<CategoryDto[]>([]);
  selectedCategoryId = signal<number | null>(null);

  // State Signals
  isLoading = signal<boolean>(true);
  isSubmitting = signal<boolean>(false);
  isDeleting = signal<boolean>(false);

  // Edit Dialog State
  isEditDialogOpen = signal<boolean>(false);
  selectedProduct = signal<ProductDto | null>(null);
  editPrice = signal<number | null>(null);
  editDiscountPrice = signal<number | null>(null);
  editDiscountStartDate = signal<string>('');
  editDiscountEndDate = signal<string>('');
  editStockQuantity = signal<number | null>(null);
  editError = signal<string>('');

  // Delete Dialog State
  isDeleteDialogOpen = signal<boolean>(false);
  productToDelete = signal<ProductDto | null>(null);

  categoryOptions = signal<{ label: string; value: number | null }[]>([]);

  ngOnInit() {
    this.loadCategories();
    this.loadProducts();
  }

  loadCategories() {
    this.categoryService.getCategories().subscribe({
      next: (cats) => {
        this.categories.set(cats);
        this.categoryOptions.set([
          { label: 'All Categories', value: null },
          ...cats.map(c => ({ label: c.name, value: c.id }))
        ]);
      },
      error: () => {
        // Fallback if categories fail to load
      }
    });
  }

  loadProducts() {
    this.isLoading.set(true);
    const categoryId = this.selectedCategoryId() ?? undefined;

    // Load up to 100 products for administration
    this.productService.getProducts(categoryId, 1, 100).subscribe({
      next: (data) => {
        this.products.set(data.items);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err.error?.detail || 'Failed to load products'
        });
      }
    });
  }

  onCategoryChange(categoryId: number | null) {
    this.selectedCategoryId.set(categoryId);
    this.loadProducts();
  }

  // --- Edit Functionality ---

  openEditDialog(product: ProductDto) {
    this.selectedProduct.set(product);
    this.editPrice.set(product.price);
    this.editDiscountPrice.set(product.rawDiscountPrice ?? product.discountPrice ?? null);
    this.editDiscountStartDate.set(this.formatForDateTimeInput(product.discountStartDate));
    this.editDiscountEndDate.set(this.formatForDateTimeInput(product.discountEndDate));
    this.editStockQuantity.set(product.stockQuantity);
    this.editError.set('');
    this.isEditDialogOpen.set(true);
  }

  closeEditDialog() {
    if (this.isSubmitting()) return;
    this.isEditDialogOpen.set(false);
    this.selectedProduct.set(null);
    this.editError.set('');
  }

  onEditVisibleChange(visible: boolean) {
    if (!visible) {
      this.closeEditDialog();
    }
  }

  submitEditProduct() {
    const product = this.selectedProduct();
    if (!product) return;

    const price = this.editPrice();
    const stock = this.editStockQuantity();
    const discountPrice = this.editDiscountPrice();
    const startDate = this.editDiscountStartDate();
    const endDate = this.editDiscountEndDate();

    if (price === null || price === undefined || price <= 0) {
      this.editError.set('Price must be greater than 0.');
      return;
    }

    if (stock === null || stock === undefined || stock < 0) {
      this.editError.set('Stock quantity cannot be negative.');
      return;
    }

    if (discountPrice !== null && discountPrice !== undefined) {
      if (discountPrice <= 0) {
        this.editError.set('Discount price must be greater than 0.');
        return;
      }
      if (discountPrice >= price) {
        this.editError.set('Discount price must be less than regular price.');
        return;
      }
      if (startDate && endDate) {
        if (new Date(startDate) > new Date(endDate)) {
          this.editError.set('Discount end date must be on or after start date.');
          return;
        }
      }
    }

    this.editError.set('');
    this.isSubmitting.set(true);

    const request: UpdateProductStockAndPriceRequest = {
      price,
      stockQuantity: stock,
      discountPrice: (discountPrice && discountPrice > 0) ? discountPrice : null,
      discountStartDate: (discountPrice && startDate) ? new Date(startDate).toISOString() : null,
      discountEndDate: (discountPrice && endDate) ? new Date(endDate).toISOString() : null
    };

    this.productService.updateProductStockAndPrice(product.id, request).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.isEditDialogOpen.set(false);
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: `Product "${product.name}" updated successfully`
        });
        this.loadProducts();
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.editError.set(err.error?.detail || 'Failed to update product');
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err.error?.detail || 'Failed to update product'
        });
      }
    });
  }

  // --- Delete Functionality ---

  openDeleteDialog(product: ProductDto) {
    this.productToDelete.set(product);
    this.isDeleteDialogOpen.set(true);
  }

  closeDeleteDialog() {
    if (this.isDeleting()) return;
    this.isDeleteDialogOpen.set(false);
    this.productToDelete.set(null);
  }

  onDeleteVisibleChange(visible: boolean) {
    if (!visible) {
      this.closeDeleteDialog();
    }
  }

  confirmDeleteProduct() {
    const product = this.productToDelete();
    if (!product) return;

    this.isDeleting.set(true);
    this.productService.deleteProduct(product.id).subscribe({
      next: () => {
        this.isDeleting.set(false);
        this.isDeleteDialogOpen.set(false);
        this.messageService.add({
          severity: 'success',
          summary: 'Deleted',
          detail: `Product "${product.name}" was successfully deleted`
        });
        this.loadProducts();
      },
      error: (err) => {
        this.isDeleting.set(false);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err.error?.detail || 'Failed to delete product'
        });
      }
    });
  }

  // Helpers
  private formatForDateTimeInput(dateStr?: string | null): string {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    if (isNaN(d.getTime())) return '';
    const pad = (n: number) => n.toString().padStart(2, '0');
    const year = d.getFullYear();
    const month = pad(d.getMonth() + 1);
    const day = pad(d.getDate());
    const hours = pad(d.getHours());
    const minutes = pad(d.getMinutes());
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }

  hasActiveDiscount(product: ProductDto): boolean {
    return product.discountPrice != null && product.discountPrice > 0;
  }
}
