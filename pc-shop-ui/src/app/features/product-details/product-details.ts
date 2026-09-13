import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { RatingModule } from 'primeng/rating';
import { TextareaModule } from 'primeng/textarea';
import { TagModule } from 'primeng/tag';
import { MessageService } from 'primeng/api';
import { ProductService } from '../../core/services/product.service';
import { CartService } from '../../core/services/cart.service';
import { AuthService } from '../../core/services/auth.service';
import { ProductDto, ProductAttributeItem } from '../../core/models/product.model';
import { ReviewDto } from '../../core/models/review.model';

@Component({
  selector: 'app-product-details',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    DatePipe,
    ButtonModule,
    TableModule,
    RatingModule,
    TextareaModule,
    TagModule
  ],
  templateUrl: './product-details.html',
  styleUrl: './product-details.css'
})
export class ProductDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private productService = inject(ProductService);
  private cartService = inject(CartService);
  public authService = inject(AuthService);
  private messageService = inject(MessageService);

  productId = signal<string>('');
  product = signal<ProductDto | null>(null);
  reviews = signal<ReviewDto[]>([]);

  isLoadingProduct = signal<boolean>(true);
  isLoadingReviews = signal<boolean>(true);
  isAddingToCart = signal<boolean>(false);
  isSubmittingReview = signal<boolean>(false);

  selectedImage = signal<string>('');

  // Review Form
  newRating = signal<number>(5);
  newComment = signal<string>('');

  // Computed gallery image list
  allImages = computed<string[]>(() => {
    const prod = this.product();
    if (!prod) return [];
    if (prod.imageUrls && prod.imageUrls.length > 0) {
      return prod.imageUrls;
    }
    if (prod.mainImageUrl) {
      return [prod.mainImageUrl];
    }
    return [];
  });

  // Computed current displayed image
  currentImage = computed<string>(() => {
    const selected = this.selectedImage();
    if (selected) return selected;
    const images = this.allImages();
    if (images.length > 0) return images[0];
    return this.product()?.mainImageUrl || '';
  });

  // Computed attributes list for p-table (handles both array and record/dictionary)
  attributesList = computed<ProductAttributeItem[]>(() => {
    const attrs = this.product()?.attributes;
    if (!attrs) return [];
    if (Array.isArray(attrs)) return attrs;
    return Object.entries(attrs).map(([key, value]) => ({ key, value: String(value) }));
  });

  // Computed average rating
  averageRating = computed<number>(() => {
    const revs = this.reviews();
    if (revs.length === 0) return 0;
    const sum = revs.reduce((acc, r) => acc + r.rating, 0);
    return Math.round((sum / revs.length) * 10) / 10;
  });

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.productId.set(id);
        this.loadProduct(id);
        this.loadReviews(id);
      }
    });
  }

  loadProduct(id: string) {
    this.isLoadingProduct.set(true);
    this.productService.getProductById(id).subscribe({
      next: (data) => {
        this.product.set(data);
        if (data.imageUrls && data.imageUrls.length > 0) {
          this.selectedImage.set(data.imageUrls[0]);
        } else if (data.mainImageUrl) {
          this.selectedImage.set(data.mainImageUrl);
        }
        this.isLoadingProduct.set(false);
      },
      error: (err) => {
        this.isLoadingProduct.set(false);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err.error?.detail || 'Failed to load product details'
        });
      }
    });
  }

  loadReviews(id: string) {
    this.isLoadingReviews.set(true);
    this.productService.getReviews(id).subscribe({
      next: (data) => {
        this.reviews.set(data);
        this.isLoadingReviews.set(false);
      },
      error: () => {
        this.isLoadingReviews.set(false);
      }
    });
  }

  selectImage(url: string) {
    this.selectedImage.set(url);
  }

  addToCart() {
    const prod = this.product();
    if (!prod) return;

    this.isAddingToCart.set(true);
    this.cartService.addToCart(prod.id, 1).subscribe({
      next: () => {
        this.isAddingToCart.set(false);
        this.cartService.loadCart();
        this.messageService.add({
          severity: 'success',
          summary: 'Added to Cart',
          detail: `${prod.name} has been added to your cart.`,
          life: 3000
        });
      },
      error: (err) => {
        this.isAddingToCart.set(false);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err.error?.detail || 'Unable to add item to cart'
        });
      }
    });
  }

  submitReview() {
    const comment = this.newComment().trim();
    if (!comment) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Validation',
        detail: 'Please enter a review comment.'
      });
      return;
    }

    const prod = this.product();
    if (!prod) return;

    this.isSubmittingReview.set(true);
    this.productService.addReview(prod.id, this.newRating(), comment).subscribe({
      next: () => {
        this.isSubmittingReview.set(false);
        this.newComment.set('');
        this.newRating.set(5);
        this.messageService.add({
          severity: 'success',
          summary: 'Review Submitted',
          detail: 'Thank you! Your review has been posted.',
          life: 3000
        });
        this.loadReviews(prod.id);
      },
      error: (err) => {
        this.isSubmittingReview.set(false);
        this.messageService.add({
          severity: 'error',
          summary: 'Review Not Submitted',
          detail: err.error?.detail || err.error?.message || 'Unable to post review. Please try again.',
          life: 5000
        });
      }
    });
  }
}
