import { Component } from '@angular/core';
import { BlurService } from '../../services/blur.service';

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss'],
})
export class NavigationComponent {
  public isOpen: boolean;

  constructor(private readonly blurService: BlurService) {
  }

  public toggle(): void {
    this.isOpen = !this.isOpen;
    this.blurService.toggleBlur();
  }

  public close(): void {
    this.isOpen = false;
    this.blurService.focusApplication();
  }
}
