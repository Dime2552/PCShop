import { Component, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { CartService } from '../../core/services/cart.service';
import { OrderService, CreateOrderRequest } from '../../core/services/order.service';

import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { RadioButtonModule } from 'primeng/radiobutton';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [FormsModule, ButtonModule, InputTextModule, RadioButtonModule],
  templateUrl: './checkout.html'
})
export class CheckoutComponent {
  cartService = inject(CartService);
  private orderService = inject(OrderService);
  private messageService = inject(MessageService);
  private router = inject(Router);

  isLoading = signal(false);
  shippingMethod = signal<string>('Standard');

  address = {
    country: '',
    city: '',
    street: '',
    zipCode: ''
  };

  // Automatically recalculate shipping cost when method changes
  shippingCost = computed(() => this.shippingMethod() === 'Express' ? 25 : 10);

  // Automatically recalculate total amount
  totalAmount = computed(() => {
    const cart = this.cartService.cartState();
    const subtotal = cart ? cart.totalCartPrice : 0;
    return subtotal + this.shippingCost();
  });

  onSubmit() {
    if (!this.address.country || !this.address.city || !this.address.street || !this.address.zipCode) {
      this.messageService.add({ severity: 'warn', summary: 'Validation', detail: 'Please fill in all address fields' });
      return;
    }

    this.isLoading.set(true);

    const payload: CreateOrderRequest = {
      shippingAddress: this.address,
      shippingMethod: this.shippingMethod()
    };

    this.orderService.createOrder(payload).subscribe({
      next: (response) => {
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Redirecting to payment...' });

        // Reload cart
        this.cartService.loadCart();

        // Redirect to Stripe Checkout
        window.location.href = response.checkoutUrl;
      },
      error: (err) => {
        this.isLoading.set(false);
        this.messageService.add({
          severity: 'error',
          summary: 'Checkout Failed',
          detail: err.error?.detail || 'An error occurred during checkout'
        });
      }
    });
  }
}