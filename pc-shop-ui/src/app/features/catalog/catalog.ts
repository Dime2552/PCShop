import { Component, inject, OnInit, signal } from '@angular/core';
import { CategoryDto } from '../../core/models/category.model';
import { PaginatedList } from '../../core/models/paginated-list.model';
import { ProductDto } from '../../core/models/product.model';
import { CategoryService } from '../../core/services/category.service';
import { ProductService } from '../../core/services/product.service';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select'; 
import { KeyValuePipe } from '@angular/common'; 
import { CartService } from '../../core/services/cart.service';

@Component({
  selector: 'app-catalog',
  imports: [ButtonModule, SelectModule, FormsModule, KeyValuePipe],
  templateUrl: './catalog.html',
  styleUrl: './catalog.css',
})
export class CatalogComponent implements OnInit {
  private categoryService = inject(CategoryService);
  private productService = inject(ProductService);
  private cartService = inject(CartService);

  // Signals
  categories = signal<CategoryDto[]>([]);
  selectedCategoryId = signal<number | null>(null);
  
  productsData = signal<PaginatedList<ProductDto> | null>(null);
  isLoading = signal<boolean>(false);

  availableFilters = signal<Record<string, string[]>>({});
  selectedFilters = signal<Record<string, string>>({});
  sortBy = signal<string>('name');

  sortOptions = [
    { label: 'By name', value: 'name' },
    { label: 'Price: Low to High', value: 'price_asc' },
    { label: 'Price: High to Low', value: 'price_desc' }
  ];

  ngOnInit() {
    this.loadCategories();
  }

  loadCategories() {
    this.categoryService.getCategories().subscribe(res => {
      this.categories.set(res);
      if (res.length > 0) {
        this.selectCategory(res[0].id);
      }
    });
  }

  selectCategory(id: number) {
    this.selectedCategoryId.set(id);
    this.selectedFilters.set({});
    this.loadFilters(id);
    this.loadProducts();
  }

  loadFilters(categoryId: number) {
    this.productService.getCategoryFilters(categoryId).subscribe(res => {
      this.availableFilters.set(res);
    });
  }

  loadProducts(page: number = 1) {
    const catId = this.selectedCategoryId();
    if (!catId) return;

    this.isLoading.set(true);
    this.productService.getProducts(
      catId, 
      page, 
      10, 
      this.sortBy(), 
      this.selectedFilters()
    ).subscribe(res => {
      this.productsData.set(res);
      this.isLoading.set(false);
    });
  }

  onFilterChange(key: string, event: Event) {
    const value = (event.target as HTMLSelectElement).value;
    const currentFilters = { ...this.selectedFilters() };
    
    if (value) {
      currentFilters[key] = value;
    } else {
      delete currentFilters[key];
    }
    
    this.selectedFilters.set(currentFilters);
    this.loadProducts(1);
  }

  onSortChange() {
    this.loadProducts(1);
  }

  addToCart(productId: string) {
    this.cartService.addToCart(productId, 1).subscribe({
      next: () => {
        this.cartService.loadCart();
        alert('Item added to cart!');
      },
      error: (err) => {
        alert(err.error.detail || 'Error at adding to cart');
      }
    });
  }
}