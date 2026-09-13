export interface OrderItemDto {
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
}

export interface OrderDto {
  orderId: string;
  totalAmount: number;
  shippingCost: number;
  status: string;
  trackingNumber?: string | null;
  createdAt: string;
  items: OrderItemDto[];
}

export interface AdminOrderDto extends OrderDto {
  userId?: string;
  customerEmail: string;
  customerName: string;
}

