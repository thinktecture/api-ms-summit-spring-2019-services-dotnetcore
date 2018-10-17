import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../../../login/services/authentication.service';
import { RealtimeService } from '../../../realtime/services/realtime.service';
import { BlurService } from '../../services/blur.service';

@Component({
  selector: 'app-root',
  templateUrl: './root.component.html',
  styleUrls: ['./root.component.scss'],
})
export class RootComponent implements OnInit {
  isBlurActive: boolean;

  constructor(
    private readonly authenticationService: AuthenticationService,
    private readonly blurService: BlurService,
    private readonly realtimeService: RealtimeService,
  ) {

  }

  ngOnInit(): void {
    this.authenticationService.activateSession();
    this.blurService.onBlurChange.subscribe(isActive => this.isBlurActive = isActive);

    this.authenticationService.sessionActivated$.subscribe(() => this.realtimeService.connect());
    this.authenticationService.sessionDeactivated$.subscribe(() => this.realtimeService.disconnect());
  }
}
