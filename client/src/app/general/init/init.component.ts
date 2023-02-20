import { ChangeDetectionStrategy, Component } from '@angular/core';
import { BehaviorSubject, forkJoin } from 'rxjs';
import { ConfigService } from 'src/app/common/config.service';

@Component({
	selector: 'kbs-init',
	templateUrl: './init.component.html',
	styles: [],
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InitComponent {
	public readonly loadingMessage = 'Laden...';
	public readonly initialization$ = new BehaviorSubject(this.loadingMessage);

	constructor(configService: ConfigService) {
		forkJoin([configService.load$()]).subscribe({
			next: () => {
				this.initialization$.next('');
			},
			error: err => {
				console.error(err);
				this.initialization$.next('Laden der Anwendung fehlgeschlagen!');
			},
		});
	}
}
