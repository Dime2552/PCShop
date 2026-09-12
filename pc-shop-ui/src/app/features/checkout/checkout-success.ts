import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CartService } from '../../core/services/cart.service';

@Component({
  selector: 'app-checkout-success',
  standalone: true,
  imports: [RouterLink, ButtonModule],
  templateUrl: './checkout-success.html'
})
export class CheckoutSuccessComponent implements OnInit {
  private cartService = inject(CartService);

  ngOnInit(): void {
    this.cartService.loadCart();
  }
}
