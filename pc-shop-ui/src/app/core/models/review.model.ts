export interface ReviewDto {
  authorName: string;
  rating: number;
  comment: string;
  createdAt: string;
  isVerifiedPurchase: boolean;
}

export interface CreateReviewRequest {
  rating: number;
  comment: string;
}
