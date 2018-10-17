import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable()
export class BlurService {
  private readonly _onBlurChange = new BehaviorSubject<boolean>(false);

  get onBlurChange(): Observable<boolean> {
    return this._onBlurChange;
  }

  toggleBlur(): void {
    this._onBlurChange.next(!this._onBlurChange.getValue());
  }

  blurApplication(): void {
    this._onBlurChange.next(true);
  }

  focusApplication(): void {
    this._onBlurChange.next(false);
  }
}
