export interface ProductAttributeItem {
  key: string;
  value: string;
}

export interface ProductDto {
  id: string;
  name: string;
  brand: string;
  price: number;
  discountPrice?: number;
  mainImageUrl: string;
  stockQuantity: number;
  description?: string;
  imageUrls?: string[];
  attributes?: ProductAttributeItem[] | Record<string, string>;
  discountStartDate?: string;
  discountEndDate?: string;
  rawDiscountPrice?: number;
}

export interface UpdateProductStockAndPriceRequest {
  price: number;
  discountPrice?: number | null;
  discountStartDate?: string | null;
  discountEndDate?: string | null;
  stockQuantity: number;
}