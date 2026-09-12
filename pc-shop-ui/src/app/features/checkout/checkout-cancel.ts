import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-checkout-cancel',
  standalone: true,
  imports: [RouterLink, ButtonModule],
  templateUrl: './checkout-cancel.html'
})
export class CheckoutCancelComponent {}
