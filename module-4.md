# Запросы
```
SELECT b.id, address_country, address_city, address_street, address_house, address_apartment, t.Name, status, "Total"
	FROM public.baskets as b left join public.time_slots as t on b.timeslot_id=t.id;

SELECT id, good_id, quantity, title, description, price, basket_id
	FROM public.items;
	
SELECT id, name, start, "end"
	FROM public.time_slots;
	
SELECT id, title, description, price, weight_gram
	FROM public.goods;	
```